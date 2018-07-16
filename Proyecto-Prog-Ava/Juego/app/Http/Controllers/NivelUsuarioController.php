<?php 


namespace App\Http\Controllers;

use  App\Models\Nivel_Usuario;
use  App\Models\Persona;
use Illuminate\Http\Request;


class NivelUsuarioController extends Controller
{

	public function listar()
	{
		$lista = Nivel_Usuario::where('puntaje','<>',0)
		->join('Persona','Nivel_Usuario.id_persona', '=','Persona.id')
		->orderBy('puntaje','des')
	    ->get();
		foreach ($lista as $item) {
			$data[]=["nombre"=>$item->nombre, 
					 "nivel"=>$item->nivel,
					 "puntaje"=>$item->puntaje];
		}
		return response()->json($data,200);
	}
}