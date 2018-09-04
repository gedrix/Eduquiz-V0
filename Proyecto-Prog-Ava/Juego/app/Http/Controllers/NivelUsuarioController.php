<?php


namespace App\Http\Controllers;

use  App\Models\nivel_usuario;
use  App\Models\persona;
use Illuminate\Http\Request;


class NivelUsuarioController extends Controller
{
	/* en el constructor, vamos a referencia a los metodos que requieren una 
	autentificacion por token
	
	*/
	public function __construct(){
	 $this->middleware('auth', ['only'=>
	 [
		 'listar',
		 'listarClasificacionJuego'
		 ]
	 ]);
	}
	/**
	 * este metodo nos permite listar los jugadores con mayor puntaje, esta lista estara visible
	 * en la pag web https://eduquiz.000webhostapp.com/ en el perfil de cada usuario, se realiza una
	 * una consulta y lo ordenamos puntaje desc y el updated_at que es una fecha modificada
	 * de manera aasc, solo mostramos datos como es: nombre, nivel, puntaje 
	 * @return type json que es toda la data
	 */
	public function listar()
	{
		$lista = nivel_usuario::where('puntaje','>',0)
		->join('persona','nivel_usuario.id_persona', '=','persona.id')
		->orderBy('puntaje','DESC')
		->orderBy('nivel_usuario.updated_at', 'ASC')
		->take(50)//solo extrae los 50 primeros
	    ->get();
		foreach ($lista as $item) {
			$data[]=["nombre"=>$item->nombre,
					 "nivel"=>$item->nivel,
					 "puntaje"=>$item->puntaje];
		}
		return response()->json($data,200);
	}

	/**
	 * En este metodo vamos a listar los jugadores como mayor puntaje pero esta lista sera visible
	 * desde la app, de igual manera se realizar una consulta para obtner los datos, nuestra conuslta
	 * estara guardad en una variable listaC, luego pasamos a preguntar si esta listaC es verdadera
	 * es decir si devuelve la consulta, creamos 4 arrays, siguiente procedemos a recorrer esta listaC
	 * con un Foreach y en cada arreglo guardamos es decir en el arreglo $dataNombre guardamos todos los 
	* nombres, por ultimo retornamos un json con cada arreglo que creamos.   
	 * @return type json con todos los datos de los arreglos
	 */
	public function listarClasificacionJuego(){
		$listaC = nivel_usuario::where('puntaje','>',0)
				->join('persona','nivel_usuario.id_persona', '=','persona.id')
				->orderBy('puntaje','DESC')
				->orderBy('nivel_usuario.updated_at', 'ASC')
				->take(50)//solo extrae los 50 primeros
				->get();
		if ($listaC) {
			$dataNombres = array();
			$dataNiveles = array();
			$dataImagenes = array();
			$dataPuntajes = array();
			foreach ($listaC as $persona) {
				$dataNombres[] = $persona->nombre;
				$dataNiveles[] = $persona->nivel; 
				$dataImagenes[] = $persona->imagen;
				$dataPuntajes[] = $persona->puntaje;
			}
			
			return response()->json(["nombres"=>$dataNombres,
				"niveles"=>$dataNiveles,
				"imagenes"=>$dataImagenes,
				"puntajes"=>$dataPuntajes,
				"mensaje"=>"Operacion existosa"], 200);
		}else{
			return response()->json(["mensaje"=>"No se ha encontrado ningun dato", "siglas"=>"NDE"], 203);
		}
	}
}
