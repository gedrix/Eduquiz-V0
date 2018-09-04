<?php 


namespace App\Http\Controllers;

use  App\Models\pregunta_reporte;
use  App\Models\persona;
use  App\Models\pregunta;
use  App\Models\categoria;
use  App\Models\preg_cate;
use Illuminate\Http\Request;


class PreguntaReporteController extends Controller
{
	/* en el constructor, vamos a referencia a los metodos que requieren una 
	autentificacion por token
	*/
	public function __construct(){
	 $this->middleware('auth', ['only'=>
	 	[
		'listarReportes',
    	'registrarReporte',
    	'EditarReporte'
		]
	 ]);
	}
	/**
	 * Este metodo nos permite listar todas preguntas que han sido reportas por el usuario, 
	 * para listar sera el reporte de pregnta siempre y cuando su id sea diferente de 0
	 * para ello vamos a listar, el motivo del reporte, el usuario quien reporto, la pregunta,
	 * la categoria y dificultad de la pregunta, Tambien listamos el external_id_reporte este nos
	 * servira para luego poder editar ese registro.
	 * @return type json la data obtenida
	 */
	public function listarReportes()
	{
	$lista = pregunta_reporte::where("pregunta_reporte.estado","<>", 0)
			->join('persona','pregunta_reporte.id_persona', '=','persona.id')
			->join('pregunta','pregunta_reporte.id_pregunta', '=','pregunta.id')
			->join('preg_cate','pregunta.id','=','preg_cate.id_pregunta')
			->join('categoria', 'preg_cate.id_categoria', '=' ,'categoria.id')
			->get();
		$data = array();
		foreach ($lista as $item) {
			$data[]=["external_id_reporte"=>$item->external_id_rep,
			         "motivo"=>$item->motivo, 
					 "usuario"=>$item->persona->nombre,
					 "id_pregunta"=>$item->id_pregunta,
					 "categoria"=>$item->nombre,
					 "dificultad"=>$item->dificultad,
					 "pregunta"=>$item->pregunta];
		}
		return response()->json($data,200);
	}

	/**
	 * en este metodo recibimos como parametros el requesy que es el json que nos envian, y el external
	 * id de la persona que realizara este reprote lo primero que se hace es preguntar si el request 
	 * recibido es un json si es asi, todo ese json lo guardamos en una variable data para poder extraer
	 * sus datos, luego realimoza una consulta para ver si el usuario que existe, si es asi, procedemos
	 * a realizar una nueva consulta con el id de pregunta este campo lo obtenemos de json $data["id_pregunta"]   
	 * luego preguntamos si el motivo existe si aasi se crea un objeto de Pregunta_Reporte y guardamos los
	 * datos obtenimos del json luego retornamos Operacion exitosa
	 * @param Request $request 
	 * @param type $external_id 
	 * @return type json mensaje de confirmacion
	 */
	public function registrarReporte(Request $request, $external_id){
        if ($request->json()) {
            $data = $request->json()->all();
            $persona = persona::where("external_id", $external_id)->first();
			if ($persona) {//el usuario debe existir
				$pregunta = pregunta::where("id", $data["id_pregunta"])->first();
				if($pregunta){//la pregunta tambien...
					if ($data["motivo"] != "") {
						$pregunta_reporte = new pregunta_reporte();
						$pregunta_reporte->motivo = $data["motivo"];
						$pregunta_reporte->persona()->associate($persona);
						$pregunta_reporte->pregunta()->associate($pregunta);
						$pregunta_reporte->external_id_rep = Utilidades\UUID::v4();
						$pregunta_reporte->save();
						return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
					}else{
						return response()->json(["mensaje"=>"Faltan datos", "siglas"=>"FD"], 203);
					}
				}else{
					return response()->json(["mensaje"=>"Pregunta no existe", "siglas"=>"PNE"], 203);
				}
            }else{
                return response()->json(["mensaje"=>"Usuario no existe", "siglas"=>"ENE"], 203);
            }
        }else{
            return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
        }
    }
    /**
     * En este metodo Editar reporte recibimos como parametro el external id del reporte
     * lo que hacemos primero es realizar uan consulta para obtener todo el objeto utilizamos el 
     * first() para obtener el primero que se encuentra y eso lo guardamos en una variable 
     * $pre_repObj, luego preguntamos si $pre_repObjsi es verdadero y realimos un find para ontener a travz
     * del id, siguiendo procedemos a cambiar el estado otra vez a 0, guardamos y retornamos Operacion exitosa 
     * @param type $external_id 
     * @return type
     */
    public function EditarReporte($external_id){
        $pre_repObj = Pregunta_Reporte::where("external_id_rep", $external_id)->first();
        if ($pre_repObj) {
            $pre_reporte =Pregunta_Reporte::find($pre_repObj->id);
            $pre_reporte->estado = 0;
            $pre_reporte->save();
             return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
        }else{
        	return response()->json(["mensaje"=>"No se encontro reporte", "siglas"=>"NSER"], 200);
    	}
    }
}