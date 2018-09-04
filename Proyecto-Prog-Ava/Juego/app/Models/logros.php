<?php 
namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class logros extends Model{
	/*referencia a la tabla de la base de datos*/
	protected $table = 'logros';
	//para saber si en la tabla usamos created_at y updated_at
	public $timestamps = false; 
    //lista blancas campos publicos
	protected $fillable = ['nombre', 'descripcion', 'imagen','external_id'];
	//lista negra campos que no quieren que se encuentren facilmente
	protected $guarded = ['id'];

	//relacion de uno a muchos
	 public function Per_Logro(){
        return $this->hasMany('App\Models\per_logro', 'id_logro');
    }
}
 ?>