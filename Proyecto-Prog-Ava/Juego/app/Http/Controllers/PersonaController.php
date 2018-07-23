<?php
namespace App\Http\Controllers;

/**
 * @author wargosh
 */
use App\Models\Persona;
use App\Models\Nivel_Usuario;
use Illuminate\Http\Request;

class PersonaController extends Controller
{
    public function Modificar(Request $request, $external_id)
    {
        //realizo la consulta de acuerdo al external id que quiero encontrar, solo el primero encontrado
        $personObj = Persona::where("external_id", $external_id)->first();
        
        if ($personObj) {
            if ($request->json()) {
                $data = $request->json()->all();//transformamos la data a json
                $persona = Persona::find($personObj->id);
                if (isset($data["nombre"])) {
                    if($data["nombre"] != "")
                        $persona->nombre = $data["nombre"];
                }
                if (isset($data["clave"])) {
                    if($data["clave"] != "")
                        $persona->clave = $data["clave"];
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
    /*
    *   Registro de usuarios
    */
    public function Registrar(Request $request){
        header('Content-Type: text/html; charset=UTF-8');
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = Persona::where("correo", $data["correo"])->first();
            if (!$persona) {
                if ($data["nombre"] != "" && $data["clave"] != "") {
                    $persona = new Persona();
                    $persona->nombre = $data["nombre"];
                    $persona->clave = $data["clave"];
                    $persona->correo = $data["correo"];
                    $persona->rol = 1;//usuario comun
                    $persona->external_id = Utilidades\UUID::v4();
                    $persona->save();
                    //retardo la ejecucion para que termine de ejecutar la sentencia en la base de datos
                    sleep(1);//retraso de 1 segundo
                    //buscar el mismo usuario creado y almaceno la demas info del jugador
                    $nivel_user = new Nivel_Usuario();
                    $nivel_user->nivel = 1;
                    $nivel_user->experiencia = 0;
                    $nivel_user->clasificacion = 0;
                    $nivel_user->puntaje = 0;
                    $nivel_user->Persona()->associate($persona);//asocio las tablas relacionadas
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
    /*
            inicio de sesion mediante json
    */
    public function Login(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $persona = Persona::where("correo", $data["correo"])
                        ->where("clave", $data["clave"])
                        ->where("estado", 1)->first();
                if ($persona) {
                    if ($persona->correo === $data["correo"] && $persona->clave === $data["clave"]) {
                        //ya no se distingue entre usuario o admin al enviar esta info porque es vital en ambos...
                        $nivel_user = Nivel_Usuario::where("id_persona", $persona->id)->first();
                        return response()->json(["correo"=>$persona->correo, 
                        "external_id"=>$persona->external_id,
                        "nombre"=>$persona->nombre,
                        "clave"=>$persona->clave,
                        "rol"=>$persona->rol,
                        "nivel"=>$nivel_user->nivel,
                        "experiencia"=>$nivel_user->experiencia,
                        "clasificacion"=>$nivel_user->clasificacion,
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

    /*
    * COMPROBAR ESTA FUNCION Y VVER SI ES FACTIBLE RECIBIR UN EXTERNAL_ID POR LA FUNCION..........
    */
    public function ActualizarNivelUsuario(Request $request, $external_id){
        $personObj = Persona::where("external_id", $external_id)->first();
        
        if ($personObj) {
            if ($request->json()) {
                $data = $request->json()->all();//transformamos la data a json
                $nivel_user = Nivel_Usuario::where("id_persona", $personObj->id)->first();//////
                if ($nivel_user) {
                    if ($nivel_user->nivel != $data["nivel"])
                        $nivel_user->nivel = $data["nivel"];
                    if ($nivel_user->experiencia != $data["experiencia"])
                        $nivel_user->experiencia = $data["experiencia"];
                    if ($nivel_user->clasificacion != $data["clasificacion"])
                        $nivel_user->clasificacion = $data["clasificacion"];
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
    public function DatosPerfil($external_id)
    {
        $personObj = Persona::where("external_id", $external_id)->first();
        if ($personObj) {
            $lista= Persona::where('external_id','=',$external_id)
                    ->join('Nivel_Usuario','Persona.id','=','Nivel_Usuario.id_persona')
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
}
