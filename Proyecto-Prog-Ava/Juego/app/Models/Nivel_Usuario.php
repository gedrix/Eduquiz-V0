<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class nivel_usuario extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'nivel_usuario';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = true;
    //lista blancas campos publicos
    protected $fillable = ['nivel', 'experiencia', 'puntaje', 'id_persona', 'created_at', 'updated_at'];
    //lista negra campos que no quieren que se encuentren facilmente
    protected $guarded = ['id'];
    
    //Relacion PERNECE A
    public function Persona(){
        return $this->belongsTo('App\Models\persona', 'id_persona');
    }
}