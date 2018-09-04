<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class pregunta extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'pregunta';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = true;
    //lista blancas campos publicos
    protected $fillable = ['pregunta', 'dificultad', 'estado', 'external_id', 'created_at', 'updated_at', 'id_persona'];
    //lista negra campos que no quieren que se encuentren facilmente
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Opciones(){
        return $this->hasMany('App\Models\opcion', 'id_pregunta');
    }
    //Relacion UNO a MUCHOS
    public function Preg_Cate(){
        return $this->hasMany('App\Models\preg_cate', 'id_pregunta');
    }
    //Relacion UNO a MUCHOS
    public function Pregunta_Reporte(){
        return $this->hasMany('App\Models\pregunta_reporte', 'id_pregunta');
    }
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Persona(){
        return $this->belongsTo('App\Models\persona', 'id_persona');
    }
}