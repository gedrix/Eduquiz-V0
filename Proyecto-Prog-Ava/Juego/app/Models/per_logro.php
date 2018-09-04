<?php 
namespace App\Models;

use Illuminate\Database\Eloquent\Model;

Class per_logro extends Model{
   /*referencia a la tabla de la base de datos*/
	protected $table = 'persona_logro';
	//para saber si en la tabla usamos created_at y updated_at
	public $timestamps = false;
	//lista blancas campos publicos
	protected $fillable = ['id_persona','id_logro'];

	//Relacion MUCHOS a UNO
	public function Logros(){
        return $this->belongsTo('App\Models\logros', 'id_logro');
    }
    //Relacion MUCHOS a UNO
    public function Persona(){
    	return $this->belongsTo('App\Models\persona', 'id_persona');
    }

}  
 ?>