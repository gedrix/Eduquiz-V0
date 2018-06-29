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
                    $persona->nombre = $data["nombre"];
                }
                if (isset($data["clave"])) {
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
                    return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
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
                        if ($persona->rol === 1) {//si es un usuario comun
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
                        }else{//si es un admin
                            return response()->json(["correo"=>$persona->correo, 
                            "external_id"=>$persona->external_id,
                            "nombre"=>$persona->nombre,
                            "clave"=>$persona->clave,
                            "rol"=>$persona->rol,
                            "token"=> base64_encode($persona->external_id.'--'.$persona->correo), 
                            "mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
                        }
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
}
