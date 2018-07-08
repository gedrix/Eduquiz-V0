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
                    return response()->json(["pregunta"=>$pregunta->pregunta,
                        "dificultad"=>$pregunta->dificultad, "categoria"=>$dataCategoria, 
                        "opcion"=>$dataOpciones,
                        "opcionEstado"=>$dataOpcionEstado,
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
}