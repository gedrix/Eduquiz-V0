<?php 

namespace App\Http\Controllers;

use  App\Models\logros;

use Illuminate\Http\Request;

/**
* 
*/
class LogrosController extends Controller
{
	/**
	 *aqui vamos referenciar los metodos que requieren una autentificacion por token 
	 */
	public function __construct(){
	 $this->middleware('auth', ['only'=>
	 [
		 'Registrar','ListarLogros','EditarLogro'
		 ]
	 ]);
	}
	/**
	 * este metodo nos permite ingresar un nuevo logro para ello lo primero
	 * que hacemos es ver si el logro a registrar ya existe, en caso de no existir
	 * pasamos verificar que no nos hayan enviando un campo vacio y guardamos el 
	 * nuevo logro con su descripcion.
	 * @param Request $request 
	 * @return type json que es un mensaje de confirmacion
	 */
	public function Registrar(Request $request)
	{
		if ($request->json()) {
			$data = $request->json()->all();
			$logro = logros::where("nombre", $data["nombre"])->first();
			if (!$logro) {
				 if ($data["nombre"] != "") {
				 	$logro = new logros();
				 	$logro->nombre = $data["nombre"];
				 	$logro->descripcion=$data["descripcion"];
				 	$logro->external_id= Utilidades\UUID::v4();
				 	$logro->save();
	                return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
				 }else{
				 	return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
				 }

			}else{
            	return response()->json(["mensaje"=>"logro ya existe", "siglas"=>"CR"], 203);
            }

		}else{
			return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
		}
	}
	/**
	 * realizamos una consulta de logros y la guardamos en una lista y obtenemos todos los datos
	 * luego creamos diferentes array's id, nombre, descrp, img y external
	 * luego recorremos la lista y vamos guardando cada dato en su respectivo arreglo 
	 * @return type json con toda los datos a presentar en la lista
	 */
	public function ListarLogros(){
		$lista = logros::get();
        $aId = array();
		$aNombre = array();
		$aDescripcion = array();
		$aImagen = array();
		$aExternal = array();
		foreach ($lista as $item) {
			$aId[] = $item->id;
			$aNombre[] = $item->nombre;
			$aDescripcion[] = $item->descripcion;	
		    $aImagen[] = $item->imagen;
			$aExternal[] = $item->external_id;
			
		}
			
		return response()->json(["ids"=>$aId,
				"nombres"=>$aNombre,
				"descripciones"=>$aDescripcion,
				"imagenes"=>$aImagen,
				"external"=>$aExternal,
				"mensaje"=>"Operacion existosa"], 200);
	}
	
	public function EditarLogro(Request $request, $external_id){
		$logroObj = logros::where("external_id", $external_id)->first();
		if ($logroObj) {
			if ($request->json()) {
				$data = $request->json()->all();

				$logroObj->nombre = $data["nombre"];
				$logroObj->descripcion = $data["descripcion"];
				$logroObj->save();

			  return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);

			}else{
                return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
            }
		}else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }
	}
}

