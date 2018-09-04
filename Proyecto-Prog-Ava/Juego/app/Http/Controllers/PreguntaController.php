<?php

namespace App\Http\Controllers;

/**
 * @author wargosh
 */
use App\Models\persona;
use App\Models\nivel_usuario;
use App\Models\pregunta;
use App\Models\categoria;
use App\Models\preg_cate;
use App\Models\opcion;
use Illuminate\Http\Request;

class PreguntaController extends Controller
{

  /* en el constructor, vamos a referencia a los metodos que requieren una 
  autentificacion por token
  
  */
  public function __construct(){
	 $this->middleware('auth', ['only'=>
	 [
		 'registrar', 'obtenerPreguntasRamdon', 'obtenerOpcionesPorIdPregunta',
     'ListaPreguntasSugueridas', 'UltimasPreguntasIngresadas', 'listarPreguntasCatyDifi',
     'ListaPreguntasSugueridasAceptar'
		 ]
	 ]);
	}

  /**
   * Este metodo registrar sirve para registrar las preguntas para ello recibimos un request
   * y un external id, lo que hacemos es preguntar si el request es json, luego transformamos
   * la data ej json, luego con el external id realizamos una consulta para saber si la persona
   * que va a registrar la pregunta existe, una vez que si existe en usuario realiziamos una nueva
   * consulta para saber si la pregunta ya existe en la base de datos, una vez que se comprueba
   * que la pregunta no existe,vemos si los campos pregunta y dificultad son diferente de vacio
   * si son pasamos a guararlos, junto con un estado de pregunta para ello usamos el campo rol de persona
   * si es rol es 0 (administrador) se guardara automaticamente con estado 1, si es rol es (0)
   * se guardara con estado 0 para que el administrador pueda revisar la pregunta
   * a continuacion se debera guardar la categoria a la que esta pregunta esta destinada
   * y las opciones guardaremos 4 en un arreglo. al final retornamos Operacion exitosa
   * @param Request $request 
   * @param type $external_id 
   * @return type json de mensaje de confirmacion
   */
    public function registrar(Request $request, $external_id){
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = persona::where("external_id", $external_id)->first();
            if ($persona) {//el usuario debe existir
                $preguntaObj = pregunta::where("pregunta", $data["pregunta"])->first();
                if (!$preguntaObj) {//si no existe
                    if ($data["pregunta"] != "" && $data["dificultad"] != "") {
                        $pregunta = new pregunta();
                        $pregunta->pregunta = $data["pregunta"];
                        $pregunta->dificultad = $data["dificultad"];
                        if ($persona->rol == 0) //si es un administrador
                            $pregunta->estado = 1;// 1 = activa | 0 - inactiva
                        else
                            $pregunta->estado = 0;// 1 = activa | 0 - inactiva
                        $pregunta->external_id = Utilidades\UUID::v4();
                        $pregunta->id_persona = $persona->id;
                        $pregunta->save();
                        //guardo a que categoria pertenece esta pregunta(por ahora solo en una categoria)
                        for ($i=0; $i < count($data["categoria"]); $i++) {
                            $categoria = categoria::where("nombre", $data["categoria"][$i])->first();
                            $preg_cate = new preg_cate();
                            $preg_cate->categoria()->associate($categoria);//asocio las tablas relacionadas
                            $preg_cate->pregunta()->associate($pregunta);
                            $preg_cate->save();
                        }
                        //guardo las (4) opciones de esta pregunta
                        for ($i=0; $i < 4; $i++) {
                            $opcion = new opcion();
                            $opcion->opcion = $data["opcion"][$i];
                            if ($i === 0)
                                $opcion->estado = "Correcta";
                            else
                                $opcion->estado = "Incorrecta";
                            $opcion->pregunta()->associate($pregunta);
                            $opcion->save();
                        }
                        return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                    }else{
                        return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
                    }
                }else{
                    return response()->json(["mensaje"=>"Pregunta ya existe", "siglas"=>"PYE"], 203);
                }
            }else{
                return response()->json(["mensaje"=>"Correo ya registrado", "siglas"=>"CR"], 203);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }

    /**
     * este metodo nos permite realizar preguntas ramdon por las diferentes categotiras y dificultades
     * para ello recibimos un request, lo primero que se hace es ver si el reques es json, luego pasamos
     * a transformar la data en json, creamos una variable auxiliar $auxDificultad, siguiente creamos un
     * switch este nos permitira obtener las preguntas segun la experiencia del jugador, una vez haber
     * obtenido el case neustra variable $auxDificultad obtendra un arreglo y en base eso se realizara
     * una consulta para obtener las preguntas dentro de la consult hacemos que sean preguntas ramdon
     * al final recorremos esa lista de preguntas que se escogio y retornamos el id, la pregunta,
     * la categoria y un mensaje de operacion exitosa
     * @param Request $request 
     * @return type json mensaje de confirmacion
     */
    public function obtenerPreguntasRamdon(Request $request){
        if ($request->json()) {
            //try {
                $data = $request->json()->all();//transformamos la data a json
                $auxDificultad = "";
                switch ($data["dificultad"]) {
                    case 'Facil':
                        $auxDificultad = ["Facil"];
                        break;
                    case 'Normal':
                        $auxDificultad = ["Facil", "Normal"];
                        break;
                    case 'Dificil':
                        $auxDificultad = ["Facil", "Normal", "Dificil"];
                        break;
                    case 'Muy Dificil':
                        $auxDificultad = ["Facil", "Normal", "Dificil", "Muy Dificil"];
                        break;
                    case 'Maestro':
                        $auxDificultad = ["Facil", "Normal", "Dificil", "Muy Dificil", "Maestro"];
                        break;
                    default:
                        $auxDificultad = ["Facil"];
                        break;
                }
                $listaPreguntas = pregunta::whereIn("dificultad", $auxDificultad)
                        ->where("estado", 1)
                        ->orderByRaw("RAND()")
                        ->take($data["cantidad"])->get();//obtiene N preguntas aleatorias
                if ($listaPreguntas) {
                    $dataIds = array();
                    $dataPreguntas = array();
                    $dataDificultad = array();
                    foreach ($listaPreguntas as $pregunta) {
                        $dataIds[] = $pregunta->id;
                        $dataPreguntas[] = $pregunta->pregunta; 
                        $dataDificultad[] = $pregunta->dificultad;
                    }
                    
                    return response()->json(["ids"=>$dataIds,
                        "preguntas"=>$dataPreguntas,
                        "dificultad"=>$dataDificultad,
                        "mensaje"=>"Operacion existosa"], 200);
                }else{
                    return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
                }
            //} catch (\Exception $exc) {
            //    return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
            //}
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
    
    /**
     * tambien sirve para listar las opciones de la pregunta..
     * este metodo nos permite obtener las opciones de la pregunta a traves de un id, para ello
     * recibimos como parametro un request, de ahi se pregunta si el request que se recibio es json
     * luego transformamos la data en json, y realizamos una consulta para obtener todos datos de esa
     * pregunta, como esta relacionado, en la tabla Opcion nos pide la id de la pregunta para obtener
     * todo un arreglo de las 4  obciones, luego recorremos toda la lista de opciones y lo iremos guardando
     * e un arreglo dataOpciones[] y su estado dataOpcionEstado[], luego recorremos la lista de categoria
     * para saber a cual o cuales categoria pertenece esa pregunta, al final retornamos, su id, opciones
     * estado y categoria, porquien fue creada esa pregunta y un mensaje de operacion Exitosa
     * @param Request $request 
     * @return type json de mensaje de confirmacion
     */
    public function obtenerOpcionesPorIdPregunta(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $pregunta = pregunta::where("id", $data["id"])->first();
                if ($pregunta) {
                    //OPTIMIZAR: esto puede reducirse a un JOIN...
                    $listaOpciones = opcion::where("id_pregunta", $pregunta->id)->get();
                    $listaCategoria = preg_cate::where("id_pregunta", $pregunta->id)->get();
                    $persona = persona::where("id", $pregunta->id_persona)->first();

                    $dataOpciones = array();
                    $dataOpcionEstado = array();
                    $dataCategoria = array();
                    foreach ($listaOpciones as $item) {
                        $dataOpciones[] = $item->opcion;
                        $dataOpcionEstado[] = $item->estado;
                    }
                    foreach ($listaCategoria as $item) {
                        $cat = categoria::where("id", $item->id_categoria)->first();
                        $dataCategoria[] = $cat->nombre;
                    }
                    return response()->json(["id"=>$pregunta->id,
                        "opcion"=>$dataOpciones,
                        "opcionEstado"=>$dataOpcionEstado,
                        "categoria"=>$dataCategoria,
                        "creadaPor"=>$persona->nombre,
                        "mensaje"=>"Operacion existosa"], 200);
                }else{
                    return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
                }
            } catch (\Exception $exc) {
                return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }

    /**
     * este metodo nos permite que el jugador pueda saber las preguntas que el a suguerido al juego
     * para ello recibimos un external id, luego realizamos una consulta para saber cual es el jugador,
     * si el jugador existe, realizamos otra consulta para obtener su id, y realizamos diferentes joins
     * para obtener los datos de, Preg_Cate, de Categoria y Pregunta, al final recorremos la lista
     * y presentamos los datos que deseamos, pregunta, nivel, categoria, tambien retornamos toda esa data
     * @param type $external_id 
     * @return type json que es la data obtenida
     */
    public function ListaPreguntasSugueridas($external_id) //preguntas que el usuario ha hecho
    {
        $personObj = persona::where("external_id", $external_id)->first();
        if ($personObj) {
          $lista = pregunta::where('id_persona','=',$personObj->id)
          ->join('preg_cate', 'pregunta.id','=','preg_cate.id_pregunta')
          ->join('categoria', 'preg_cate.id_categoria', '=' ,'categoria.id')
          ->orderBy('created_at','des')
          ->get();
          $data = array();
          foreach ($lista as $item) {
              $data[]=["pregunta"=>$item->pregunta,
                       "nivel"=>$item->dificultad,
                       "categoria"=>$item->nombre
                       ];
          }
         return response()->json($data, 200);
        }else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
    }
    /**
     * este metodo solo nos permitira ver cuales fueron las 10 ultimas preguntas ingresadasal a la base
     * de datos para ello solo realizamos una consulta siempre y cuando su estado sea 1, y con varios Join
     * para referenciar a la Categoria que pertenece esa pregunta, recorremos la lista y presentamos
     * pregunta, nivel, categoria, y al final retornamos esa lista
     * @return type json que es la data obtenida
     */
    public function UltimasPreguntasIngresadas(){
        $lista = pregunta:: where('estado','=',1)
                 ->join('preg_cate', 'pregunta.id','=','preg_cate.id_pregunta')
                 ->join('categoria', 'preg_cate.id_categoria', '=' ,'categoria.id')
                 ->orderBy('created_at','des')
                 ->take(10)
                 ->get();
        foreach ($lista as $item) {
          $data[]=["pregunta"=>$item->pregunta,
                   "nivel"=>$item->dificultad,
                   "categoria"=>$item->nombre
                  ];
          }
         return response()->json($data,200);
    }

    /**
     * Este metodo sera utilzado en la pag web en la parte de administrador para poder obtener
     * las preguntas segun por categoria y dificultad, para ello recibimos un request, preguntamos
     * si el request es json, luego transformamos la data en json, y guardamos en 2 variables 
     * la dificultad y categoria que recibimos por el request, y realizamos una consulta para
     * obtener las lista de preguntas segun la categoria y dificultad ingresa, luego recorremos esa lista
     * y prsentamos el id, pregunta, nivel, categoria y retornamos esta lista.
     * @param Request $request 
     * @return type json que es la data obtenida
     */
    public function listarPreguntasCatyDifi(Request $request){
        if ($request->json()) {
        try {
          $data = $request->json()->all();
          $dificultad = $data["dificultad"];
          $categoria  = $data["categoria"];
          $lista = Pregunta:: where('pregunta.dificultad','=',$dificultad)
                            ->where('categoria.nombre','=',$categoria)
                 ->join('preg_cate', 'pregunta.id','=','preg_cate.id_pregunta')
                 ->join('categoria', 'preg_cate.id_categoria', '=' ,'categoria.id')

                 ->get();
          foreach ($lista as $item) {
           $data[]=["id"=>$item->id_pregunta,
                    "pregunta"=>$item->pregunta,
                    "nivel"=>$item->dificultad,
                    "categoria"=>$item->nombre
                  ];
          }
         return response()->json($data,200);
        } catch (\Exception $exc) {
          return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
        }
      }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }

      
    }

    /**
     * en este metodo listaremos todas las preguntas que han sido sugeridas por los jugadores y el/los
     * administradores deberan verlas para luego aceptarlas o rechazarlas para ello realizamos una consulta
     * siempre y cuando el estado de la pregunta sea igual a cero, realizamos un join para saber que jugador
     * realizo esa pregunta, tambien un jon con pre_cate para saber a que categoria pertenece, luego 
     * recorremos la lista y lo vamos guarndando en un arreglo, y al final retornamos ese arreglo. 
     * @return type json que es la data obtenida
     */
    public function ListaPreguntasSugueridasAceptar(){  //lista de preguntas del usuario hacia el admin
        $lista = pregunta::where('pregunta.estado','=',0)
                 ->join('persona','pregunta.id_persona','=','persona.id')
                 ->join('preg_cate', 'pregunta.id','=','preg_cate.id_pregunta')
                 ->join('categoria', 'preg_cate.id_categoria', '=' ,'categoria.id')
                 ->get();
        foreach ($lista as $item) {
            $data[]= [ "usuario"=>$item->persona->nombre,
                       "pregunta"=>$item->pregunta,
                       "nivel"=>$item->dificultad,
                       "id_pregunta"=>$item->id_pregunta,
                       "categoria"=>$item->nombre,
                     ];
             }
            return response()->json($data,200);

    }

    /**
     * este metodo nos permite editar la pregunta con sus respectivas opciones, lo que recibimos
     * es un requet, preguntamos si el request recibido es un json, luego transformamos la data en
     * json, de la data sacamos el id de pregunta, obtenemos el objeto de pregunta a travez de una 
     * consulta y sacamos cual es la pregunta y la dificultado que tiene, y el estado siempre que
     * se edite estara en 1, luego realizamos una nueva consulta para guarda las opciones de esa pregunta
     * para ello primero sacamos las opciones que existen y las reemplzamos con la data recibida
     * editando a traves de un arreglo y guardamos. Luego nos faltataria editar la categoria de esa pregunta
     * como sabemso que una pregunta puede tener una o varias categorias para esto eliminamos la el 
     * campo que tenga la pregunta con su actual categria, para luevo recorrer un for con la cantidad
     * de categorias que se desea ingesar y guardamos la categoria, al final retornamos
     * operacion exitosa
     * @param Request $request 
     * @return type json mensaje de confirmacion
     */
    public function editarPregunta(Request $request){
     if ($request->json()) {
      try {
        $data = $request->json()->all();

        $id_preg = $data["id"];
        $preguntaObj = pregunta::where("id", $id_preg)->first();
        if ($preguntaObj) {
             $pregunta =pregunta::find($preguntaObj->id);
             $pregunta->pregunta = $data["pregunta"];
             $pregunta->dificultad = $data["dificultad"];
             $pregunta->estado = 1;
             $listaOpciones = opcion::where("id_pregunta", $id_preg)->get();
             if($listaOpciones){
                $i = 0;
                foreach($listaOpciones as $item){
                    $opcion = opcion::find($item->id);
                    $opcion->opcion = $data["opcion"][$i];
                    $opcion->save();
                    $i++;
                }
                $pregunta->save();
                preg_cate::where("id_pregunta","=", $id_preg)->delete();

                for ($i=0; $i < count($data["categoria"]); $i++) { 
                  $categoria = categoria::where("nombre", "=", $data["categoria"][$i])->first();
                
                  $preg_cate = new preg_cate();
                  $preg_cate->id_pregunta = $id_preg;
                  $preg_cate->id_categoria = $categoria->id;
                  $preg_cate->save();
                }

                return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
             }else{
                return response()->json(["mensaje"=>"No se encontro opciones", "siglas"=>"NSEO"], 200);
             }
              
        }else{
          return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
       }


      } catch (\Exception $exc) {
         return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
      }
     }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
}
