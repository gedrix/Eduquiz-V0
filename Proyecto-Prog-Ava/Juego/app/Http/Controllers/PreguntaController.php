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
    public function registrar(Request $request){
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = Persona::where("correo", $data["correo"])->first();
            if ($persona) {
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
                        return response()->json(["mensaje"=>"Operacion existosa",
                            "external_id"=>$pregunta->external_id, "siglas"=>"OE"], 200);
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

    //NOTA:     Este codigo se puede optimizar las consultas en solo uno o dos whereHas...
    public function obtenerPreguntaRamdon(Request $request){
        if ($request->json()) {
            try {
                $data = $request->json()->all();//transformamos la data a json
                $pregunta = Pregunta::where("dificultad", $data["dificultad"])
                        ->where("estado", 1)
                        ->orderByRaw("RAND()")->first();
                if ($pregunta) {
                    $listaOpciones = Opcion::where("id_pregunta", $pregunta->id)->get();
                    $listaCategoria = Preg_Cate::where("id_pregunta", $pregunta->id)->get();
                    /*$listaCategoria = Categoria::whereHas('Preg_Cate', function($q){
                        $q->where('id_categoria', $idCategoria);
                    })->first();*/
                    $dataOpciones = array();
                    $dataCategoria = array();
                    foreach ($listaOpciones as $item) {
                        $dataOpciones[] = ["opcion" => $item->opcion, 
                            "estado" => $item->estado];
                    }
                    foreach ($listaCategoria as $item) {
                        $cat = Categoria::where("id", $item->id_categoria)->first();
                        $dataCategoria[] = ["nombre" => $cat->nombre];
                    }
                    return response()->json(["pregunta"=>$pregunta->pregunta,
                        "dificultad"=>$pregunta->dificultad, "categorias"=>$dataCategoria, 
                        "opciones"=>$dataOpciones], 200);
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