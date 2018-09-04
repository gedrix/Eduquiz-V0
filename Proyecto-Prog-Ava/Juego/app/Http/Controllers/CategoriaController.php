<?php

namespace App\Http\Controllers;

use  App\Models\categoria;

use Illuminate\Http\Request;

/**
*
*/
class CategoriaController extends Controller
{

		////****************AUNTENTICATE*************************

	/**
	 *aqui vamos referenciar los metodos que requieren una autentificacion por token 
	 */
	public function __construct(){
	 $this->middleware('auth', ['only'=>
	 [
		 'Registrar', 'EditarCategoria', 'ListarCategoria'
		 ]
	 ]);
	}

	/**
	 * el mentodo registrar recibe un un json, 
	 *si primero preguntamos i lo que recibimos es un json si es asi realizamos una consulta
	 *apara saber si esa categoria existe, si no existe, para poder guardar, guadamos el nombre
	 *y un UUID como external id, si too esta bien retornamos operacion exitosa,
	 *caso contrario retornamos faltan datos en el formulario
	 * @param Request $request 
	 * @return type json mensaje de confirmación
	 */
	public function Registrar(Request $request)
	{
		if ($request->json()) {
			$data = $request->json()->all();
			$categoria = categoria::where("nombre", $data["nombre"])->first();
			if (!$categoria) {
				 if ($data["nombre"] != "") {
				 	$categoria = new categoria();
				 	$categoria->nombre = $data["nombre"];
				 	$categoria->external_id= Utilidades\UUID::v4();
				 	$categoria->save();
	                return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);
				 }else{
				 	return response()->json(["mensaje"=>"Faltan datos en formulario", "siglas"=>"FDEF"], 203);
				 }

			}else{
            	return response()->json(["mensaje"=>"categoria ya existe", "siglas"=>"CR"], 203);
            }

		}else{
			return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
		}
	}
	/**
	 * En este metodo recibimos en json los datos a ser editados,y el external external id, 
	 *	para esto realizamos desde un metodo post, lo primero que se hace es objteer todo el objeto
	 *	de categoria para saber cual vamos a editar,
     * 	si existe, luego pasamos a preguntar si el dato que recibimos (request) es json si es asi, 
	 *	procedemos a guardar el nuevo dato y retornamos un mensaje 'Operacion exitosa'
	 * @param Request $request 
	 * @param type $external_id 
	 * @return type json que es un mensaje de confirmacion
	 */
	public function EditarCategoria(Request $request, $external_id)
	{
		$categoriaObj = categoria::where("external_id", $external_id)->first();
		if ($categoriaObj) {

			if ($request->json()) {

				$data = $request->json()->all();

                $categoriaObj->nombre = $data["nombre"];
                $categoriaObj->save();
	            return response()->json(["mensaje"=>"Operacion existosa", "siglas"=>"OE"], 200);



			}else{
                return response()->json(["mensaje"=>"La data no tiene el formato deseado", "siglas"=>"DNF"], 400);
            }

		}else{
            return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
        }

	}
	/**
	 * este metodo nos permite listar todas las categorias, para ello solo realimos una consulta
	 * para obtener una lista de objetos y luego recorremos esa lista e ir presentando categoria por 
	 * categoria, tambien devolvemos el external id para que con este campo poder llamar al editar 
	 * Categoria
	 * @return type json que es la data
	 */
	public function ListarCategoria(){
		$lista =categoria::
		        //->orderBy('id')
			    get();
		foreach ($lista as $item) {
			$data[]=["categoria"=>$item->nombre,
			         "external_id" =>$item->external_id ] ;
			
		}
		return response()->json($data,200);
	}

}
