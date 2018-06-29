<?php 
namespace App\Http\Controllers;

use  App\Models\Pregunta;

use Illuminate\Http\Request;

/**
* 
*/
class PreguntaController extends Controller
{
	
	public function Registrar(Request $request)
	{
		if ($request->json()) {
			$data = $request->json()->all();
            $pregunta = Pregunta::where("pregunta", $data["pregunta"])->first();

            if (!$pregunta) {
            	 if ($data["pregunta"] != "" && $data["pregunta"] != "") {
            	 	$pregunta = new Pregunta();
	            	$pregunta->pregunta = $data["pregunta"];
	                $pregunta->dificultad = $data["pregunta"];
	                $pregunta->estado = "activo";
	                $pregunta->external_id= Utilidades\UUID::v4();
	                $pregunta->id_persona = 1;
	                $pregunta->save();
	                return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
	             }else{
	             	return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
	             }    
            }else{
            	return response()->json(["mensaje"=>"pregunta ya registrada", "siglas"=>"CR"], 203);
            }
		}else{
			return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
		}
	}


}

