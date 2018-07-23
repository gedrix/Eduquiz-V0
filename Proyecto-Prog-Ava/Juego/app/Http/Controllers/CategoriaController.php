<?php 

namespace App\Http\Controllers;

use  App\Models\Categoria;

use Illuminate\Http\Request;

/**
* 
*/
class CategoriaController extends Controller
{
	
	public function Registrar(Request $request)
	{
		if ($request->json()) {
			$data = $request->json()->all();
			$categoria = Categoria::where("nombre", $data["nombre"])->first();
			if (!$categoria) {
				 if ($data["nombre"] != "") {
				 	$categoria = new Categoria();
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

	public function EditarCategoria(Request $request, $external_id)
	{
		$categoriaObj = Categoria::where("external_id", $external_id)->first();
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

	public function ListarCategoria(){
		$lista =Categoria::
		        //->orderBy('id')
			    get();
		foreach ($lista as $item) {
			$data[]=["categoria"=>$item->nombre ] ;
		}
		return response()->json($data,200);
	}
	
}