<?php

namespace App\Http\Controllers;

/**
 * @author wargosh
 */
use App\Models\Persona;
use App\Models\Nivel_Usuario;
use App\Models\Pregunta;
use App\Models\Categoria;
use App\Models\Preg_Cate;
use App\Models\Opcion;
use Illuminate\Http\Request;

class PreguntaController extends Controller
{
    public function registrar(Request $request, $external_id){
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = Persona::where("external_id", $external_id)->first();
            if ($persona) {//el usuario debe existir
                $preguntaObj = Pregunta::where("pregunta", $data["pregunta"])->first();
                if (!$preguntaObj) {//si no existe
                    if ($data["pregunta"] != "" && $data["dificultad"] != "") {
                        $pregunta = new Pregunta();
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
                            $categoria = Categoria::where("nombre", $data["categoria"][$i])->first();
                            $preg_cate = new Preg_Cate();
                            $preg_cate->Categoria()->associate($categoria);//asocio las tablas relacionadas
                            $preg_cate->Pregunta()->associate($pregunta);
                            $preg_cate->save();
                        }
                        //guardo las (4) opciones de esta pregunta
                        for ($i=0; $i < 4; $i++) { 
                            $opcion = new Opcion();
                            $opcion->opcion = $data["opcion"][$i];
                            if ($i === 0)
                                $opcion->estado = "Correcta";
                            else
                                $opcion->estado = "Incorrecta";
                            $opcion->Pregunta()->associate($pregunta);
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

    public function obtenerPreguntasRamdon(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $listaPreguntas = Pregunta::where("dificultad", $data["dificultad"])
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
            } catch (\Exception $exc) {
                return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
    public function obtenerOpcionesPorIdPregunta(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $pregunta = Pregunta::where("id", $data["id"])->first();
                if ($pregunta) {
                    //OPTIMIZAR: esto puede reducirse a un JOIN... 
                    $listaOpciones = Opcion::where("id_pregunta", $pregunta->id)->get();
                    $listaCategoria = Preg_Cate::where("id_pregunta", $pregunta->id)->get();
                    $persona = Persona::where("id", $pregunta->id_persona)->first();

                    $dataOpciones = array();
                    $dataOpcionEstado = array();
                    $dataCategoria = array();
                    foreach ($listaOpciones as $item) {
                        $dataOpciones[] = $item->opcion; 
                        $dataOpcionEstado[] = $item->estado;
                    }
                    foreach ($listaCategoria as $item) {
                        $cat = Categoria::where("id", $item->id_categoria)->first();
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
    public function ListaPreguntasSugueridas($external_id)
    {
        $personObj = Persona::where("external_id", $external_id)->first();
        if ($personObj) {
          // return response()->json(["id"=>$personObj->id,
          //  "nombre"=>$personObj->nombre
          //  ],200);
          $lista = Pregunta::where('id_persona','=',$personObj->id)
          ->join('Preg_Cate', 'Pregunta.id','=','Preg_Cate.id_pregunta')
          ->join('Categoria', 'Preg_Cate.id_categoria', '=' ,'Categoria.id')
          ->orderBy('created_at','des')
          ->get();
          foreach ($lista as $item) {
              $data[]=["pregunta"=>$item->pregunta,
                       "nivel"=>$item->dificultad,
                       "categoria"=>$item->nombre
                       ];
          }
         return response()->json($data,200);
        }else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
    }
    
    public function UltimasPreguntasIngresadas(){
        $lista = Pregunta:: where('estado','=',1)
                 ->join('Preg_Cate', 'Pregunta.id','=','Preg_Cate.id_pregunta')
                 ->join('Categoria', 'Preg_Cate.id_categoria', '=' ,'Categoria.id')
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
    

    public function listarPreguntasCatyDifi($dificultad,$categoria){
        $lista = Pregunta:: where('Pregunta.dificultad','=',$dificultad)
                            ->where('Categoria.nombre','=',$categoria)
                 ->join('Preg_Cate', 'Pregunta.id','=','Preg_Cate.id_pregunta')
                 ->join('Categoria', 'Preg_Cate.id_categoria', '=' ,'Categoria.id')
                 
                 ->get();
        foreach ($lista as $item) {
          $data[]=["pregunta"=>$item->pregunta,
                   "nivel"=>$item->dificultad,
                   "categoria"=>$item->nombre                   
                  ];
          }
         return response()->json($data,200);
    }

    public function ListaPreguntasSugueridasAceptar(){
        $lista = Pregunta::where('Pregunta.estado','=',0)
                 ->join('Persona','Pregunta.id_persona','=','Persona.id')
                 ->join('Preg_Cate', 'Pregunta.id','=','Preg_Cate.id_pregunta')
                 ->join('Categoria', 'Preg_Cate.id_categoria', '=' ,'Categoria.id')
                 ->join('Opcion','Pregunta.id','=','Opcion.id_pregunta')
                 ->get();
        foreach ($lista as $item) {
            $data[]= [ "usuario"=>$item->Persona->nombre,
                       "pregunta"=>$item->pregunta,
                       "nivel"=>$item->dificultad,
                       
                       "categoria"=>$item->nombre,
                       "opcion"=>$item->opcion,
                       "Repuesta"=>$item->estado

                     ];
             }
            return response()->json($data,200);
       
    }
}