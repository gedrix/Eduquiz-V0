<?php
namespace App\Http\Controllers;

/**
 * @author wargosh
 */
use App\Models\persona;
use App\Models\nivel_usuario;
use App\Models\logros;
use App\Models\per_logro;
use Illuminate\Http\Request;

class PersonaController extends Controller
{
    /*
    aqui vamos referenciar los metodos que requieren una autentificacion por token
    */
  public function __construct(){
	 $this->middleware('auth', ['only'=>
	 [
		 'Modificar', 'ActualizarNivelUsuario', 'DatosPerfil','ListaUsuarios','ListarLogrosPersona',
         'RegistrarLogro'
		 ]
	 ]);
	}
    /**
     * en este metodo Modificar recibimos como parametros el request y el external id de la
     * persona a modificar este metodo será utilizado tanto el app como en la pag web, realizo la 
     * consulta de acuerdo al external id que quiero encontrar, solo el primero encontrado. Luego 
     * realizamos un find para de personaObj,para modificar el nombre del usuario preguntamos 
     * isset($data["nombre"]) para saber si esta inicializado y sobre todo si no esta vacia, 
     * si es asi se cambiara el nombre, en caso de querer cambiar la clave se preguntara de la
     * misma forma por el isset($data[clave]) al finalizar guardamos y retornamos Operacion exitosa
     * @param Request $request 
     * @param type $external_id 
     * @return type json mensaje de cofirmacion
     */
    public function Modificar(Request $request, $external_id)
    {
        //realizo la consulta de acuerdo al external id que quiero encontrar, solo el primero encontrado
        $personObj = persona::where("external_id", $external_id)->first();

        if ($personObj) {
            if ($request->json()) {
                $data = $request->json()->all();//transformamos la data a json
                $persona = persona::find($personObj->id);
                if (isset($data["nombre"])) {
                    if($data["nombre"] != "")
                        $persona->nombre = $data["nombre"];
                }
                if (isset($data["clave"])) {
                    if($data["clave"] != "")
                        $persona->clave = sha1($data["clave"]."EDUquizKey3.");
                }
                 if (isset($data["imagen"])) {
                    if($data["imagen"] != "")
                        $persona->imagen = $data["imagen"];
                }
                $persona->save();
                return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
            }else{
                return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
            }
        }else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
    }

    /**
       * Registro de usuarios este metodo podra ser utilizado tanto en la app como en la pag web,
       * recibimos un request, preguntamos si el request es json, luego transformamos la data a json
       * como en nuestro caso para registrar una persona es por correo su correo debe de ser unico
       * por eso realizamos una consulta para ver si ese correo existe en caso de que no exista
       * pasamos a verificar que el nombre y clave que recibimos sean diferentes de vacio, si es asi
       * vamos a guardar nombre y clave, para la clave usamos el metodo de encriptamiento sha1
       * + la clave y algo adicional el nombre de nuestro proeycto "EDUquizKey3.", de igual forma
       * se guarda un external_id hasta ahi guardamos los datos persona y damos un sleep(1) que es un
       * tiempo de espera de 1 segundo, tambien necesitamos guardar un nivel de usuario ya que esto
       * sera el nivel en el que empezara y la experiencia con la clasificacion empecerá en 0
       * al guardar todo retornamos un mensaje Operacion exitosa
         * @param Request $request 
         * @return type json mensaje de confirmacion
         */    
    public function Registrar(Request $request){
        header('Content-Type: text/html; charset=UTF-8');
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = persona::where("correo", $data["correo"])->first();
            if (!$persona) {
                if ($data["nombre"] != "" && $data["clave"] != "") {
                    $persona = new persona();
                    $persona->nombre = $data["nombre"];
                    $clave = sha1($data["clave"]."EDUquizKey3.");
                    $persona->clave = $clave;
                    $persona->correo = $data["correo"];
                    $persona->rol = 1;//usuario comun
                    $persona->external_id = Utilidades\UUID::v4();
                    $persona->save();
                    //retardo la ejecucion para que termine de ejecutar la sentencia en la base de datos
                    sleep(1);//retraso de 1 segundo
                    //buscar el mismo usuario creado y almaceno la demas info del jugador
                    $nivel_user = new nivel_usuario();
                    $nivel_user->nivel = 1;
                    $nivel_user->experiencia = 0;
                    //$nivel_user->clasificacion = 0;
                    $nivel_user->puntaje = 0;
                    $nivel_user->persona()->associate($persona);//asocio las tablas relacionadas
                    $nivel_user->save();
                    return response()->json(["mensaje"=>"Operacion existosa", "nombre"=>$data["nombre"], "siglas"=>"OE"], 200);
                }else{
                    return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
                }
            }else{
                return response()->json(["mensaje"=>"Correo ya registrado", "siglas"=>"CR"], 203);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
    /**
     *Este metodo Login nos permitira logear desde la aplicacion movil y desde la pag web
     * para ello este metodo será a travez del post ya que recibiremos un request, lo primero
     * que haces es ver si el request es json, luego transformamos la data a json,
     * luego realizamos una consulta para saber si el correo que desea logear existe en la BD
     * para verificar la clave de igual manera debemos de compararla encriptada, y por ultimo
     * vemos el estado si el estado 1 que refiere a jugador activo este le permitira logear
     * al final retornamos varios datos que nos serviran tanto en al app como en la
     * pag web
     * @param Request $request 
     * @return type json mensaje de confirmacion
     */
    
    public function Login(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $clave = sha1($data["clave"]."EDUquizKey3.");
                $persona = persona::where("correo", $data["correo"])
                        ->where("clave", $clave)
                        ->where("estado", 1)->first();
                if ($persona) {
                    if ($persona->correo === $data["correo"] && $persona->clave === $clave) {
                        //ya no se distingue entre usuario o admin al enviar esta info porque es vital en ambos...
                        $nivel_user = nivel_usuario::where("id_persona", $persona->id)->first();
                        return response()->json(["correo"=>$persona->correo,
                        "external_id"=>$persona->external_id,
                        "nombre"=>$persona->nombre,
                        "imagen"=>$persona->imagen,
                        "rol"=>$persona->rol,
                        "nivel"=>$nivel_user->nivel,
                        "experiencia"=>$nivel_user->experiencia,
                        "puntaje"=>$nivel_user->puntaje,
                        "token"=> base64_encode($persona->external_id.'--'.$persona->correo),
                        "mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                    }else{
                        return response()->json(["mensaje"=>"Incompatibilidad de datos", "siglas"=>"IDD"], 203);
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

    /**
     * Este sera utilizado principalmente en al app, para ir actualizando el nivel, experiencia,
     * puntaje, para ello recibimos como parametro un request, y el external id del jugador, 
     * para ello se realiza una consulta con el external  id para saber cual es el jugador que se
     * va a actualizar, luego vemos si request tiene el formato json luego transformamos la data
     * a json y aqui vamos a actualizar los datos de Nivel Usuario con el id de la persona,
     * luego preguntmos si el nivel actual es diferente al nivel que se va actualizar, si es diferente
     * este lo guardara y asi con los demas campos, al final retornamos Operacion exitosa 
     * @param Request $request 
     * @param type $external_id 
     * @return type json mensaje de confirmacion
     */
    
    public function ActualizarNivelUsuario(Request $request, $external_id){
        $personObj = persona::where("external_id", $external_id)->first();

        if ($personObj) {
            if ($request->json()) {
                $data = $request->json()->all();//transformamos la data a json
                $nivel_user = nivel_usuario::where("id_persona", $personObj->id)->first();//////
                if ($nivel_user) {
                    if ($nivel_user->nivel != $data["nivel"])
                        $nivel_user->nivel = $data["nivel"];
                    if ($nivel_user->experiencia != $data["experiencia"])
                        $nivel_user->experiencia = $data["experiencia"];
                    if ($nivel_user->puntaje != $data["puntaje"])
                        $nivel_user->puntaje = $data["puntaje"];
                    $nivel_user->save();
                    return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                }
                return response()->json(["mensaje"=>"No se ha encontrado la informacion del usuario", "siglas"=>"NEIU"], 203);
            }else{
                return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
            }
        }else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
    }

    /**
     * Este metodo sera mayormente utilizado en la pag web, cuando la persona o jugador se logee este
     * podra ver sus datos, como son nombre suyo, nivel, experiencia. para ello se recibe como parametro el
     * external id, lo que hace con ese external hacer una consulta para obtener el objeto de la persona,
     * luego volvemos a hacer una consulta para extraer los datos que necesitamos presentar,
     * por ultimo recorremos la lista y retornamos el json 
     * @param type $external_id 
     * @return type json con toda la data que se recorrio en la lista
     */
    public function DatosPerfil($external_id)
    {
        $personObj = persona::where("external_id", $external_id)->first();
        if ($personObj) {
            $lista= persona::where('external_id','=',$external_id)
                    ->join('nivel_usuario','persona.id','=','nivel_usuario.id_persona')
            ->get();
            foreach ($lista as $item) {
                $data[]=["nombre"=>$item->nombre,
                         "nivel"=>$item->nivel,
                          "experiencia"=>$item->experiencia];
            }
            return response()->json($data,200);
        }else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
    }

    /**
     * Este metodo nos permite realizar una lista de todos los usuarios, para ello se realiza
     * una consulta que vamos a mostrar los usuarios cuyo rol sea diferete de cero
     * para esto solo mostraremos 10, esto lo guardamos en una variable, luego recorremos esa 
     * variable y vamos guardando en un arreglo $data[] y vmos guardado, al final retornamos 
     * ese arreglo $data[]
     * @return type json con toda la data que se recorrio en la lista
     */
    public function ListaUsuarios(){
       
        $lista = Persona::where("persona.rol","<>",0)
        ->get();
        $data = array();     
        foreach ($lista as $item) {
          $data[]=[ 
                    "nombre"=>$item->nombre,
                    "correo" =>$item->correo,
                    "external" =>$item->external_id,
                    "estado"=>$item->estado
                   ];
        }
             return response()->json($data,200);
    }

    /**
     * esta funcion es principalmente para la pag web seccion administrador que realizara una 
     * busqueda filtrata de nombre de personas, para ello recibimos como parametro un request
     * para ello realizamos una consulta con el like tanto en la derecha como izquierda para que sea
     * una busqueda filtrada, y al final recoremos esa lista para guardarla en un arreglo
     * y retornar ese arreglo
     * @param Request $request 
     * @return type json con toda la data que se recorrio en la lista
     */
    public function buscarUsuario(Request $request){
        if ($request->json()) {
            $data = $request->json()->all();
            $lista = persona::where("persona.rol","<>",0)
            //->where("Persona.correo","like", $data["buscar"]."%")
            ->where("persona.nombre","like", "%". $data["buscar"] ."%")
            ->get();
            $data = array();     
             foreach ($lista as $item) {
               $data[]=[ 
                    "nombre"=>$item->nombre,
                    "correo" =>$item->correo,
                    "external" =>$item->external_id,
                    "estado"=>$item->estado
                   ];
        }
        return response()->json($data,200);
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }

        
    }

    /**
     * este metodo sera utilizado en la pag web en la seccion de administrador, sirve 
     * para que el administrador pueda banear y quitar el baneado a una persona es decir
     * inabilitar y habilitar, para ello recibimos como parametro un request, 
     * y a travez del id recializamos una consulta y sobre escribimos su estado 
     * y al final retornamos Operacion exitosa
     * @param Request $request 
     * @param type $external_id 
     * @return type json que es un mensaje de confirmacion
     */
    public function EditarEstadoUsuario(Request $request,$external_id){
        if ($request->json()) {
            try {
                $data = $request->json()->all();
                //$id_per = $data["id"];
                 $personaObj = persona::where("external_id", $external_id)->first();
                 if ($personaObj) {
                    $persona =persona::find($personaObj->id);
                    $persona->estado= $data["estado"];
                    $persona->save();
                     return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                 }else{
                return response()->json(["mensaje"=>"No se encontro persona", "siglas"=>"NSEP"], 200);
             }
       
            } catch(\Exception $exc) {
                return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 400);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }

    /**
     * en este metodo vamos a listar todos los logros obtenidos por el usuario
     * para ello realizamos una consulta y la guardamos en una variable lista
     * nuestra consulta tiene 2 joins para hacer referencia a nuestra tabla
     * per_logro que es quien tiene el id de la persona y el de logro, luego creamos
     * 2 arrays de nombre y descripcion para ir guardando cada dato
     * y eso es lo que vamos a retornar
     * @param type $external_id 
     * @return type json con toda la data que se recorrio en la lista
     */
    public function ListarLogrosPersona($external_id){
        $lista = per_logro::where("persona.external_id", $external_id)
        ->join('persona', 'persona_logro.id_persona','=','persona.id')
        ->join('logros','persona_logro.id_logro','=','logros.id')
        ->get();
         //$lista
        $aNombre = array();
        $aDescripcion = array();
        foreach ($lista as $item) {
           $aNombre[] = $item->logros->nombre;
           $aDescripcion[] = $item->descripcion;             
        }
        return response()->json([
            "nombres"=>$aNombre,
            "descripciones"=>$aDescripcion,
            "mensaje"=>"Operacion existosa"], 200);
    }

    /**
     * Este metdo nos permitirá rgistar el logro que la persona obtiene
     * en nuestra tabla per_logro, para ello recibimos un request con los 
     * datos de persona, y verificamos a traves de una consulta el external_id
     * si la persona existe guardamos en per_logr el id de la persona y el id del logro
     * al final retornamos un mensaje de confirmacion
     * @param Request $request 
     * @return type json que es un mensaje de confirmacion
     */
    public function RegistrarLogro(Request $request){
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = persona::where("external_id", $data["external_id"])->first();
            if ($persona) {
                if ($data["id_logro"] != "") {
                    $per_logro = new per_logro();
                    $per_logro->id_persona = $persona->id;
                    $per_logro->id_logro = $data["id_logro"];
                    $per_logro->save();
                    return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                }else{
                    return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
                }
            }else{
                return response()->json(["mensaje"=>"Usuario no registrado", "siglas"=>"CR"], 203);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
}
