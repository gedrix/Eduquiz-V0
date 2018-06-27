<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Pregunta extends Model{
    protected $table = 'pregunta';
    public $timestamps = true;
    //lista blancas
    protected $fillable = ['pregunta', 'dificultad', 'estado', 'external_id', 'created_at', 'updated_at', 'id_persona'];
    //lista negra
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Opciones(){
        return $this->hasMany('App\Models\Opcion', 'id_pregunta');
    }
    //Relacion UNO a MUCHOS
    public function Preg_Cate(){
        return $this->hasMany('App\Models\Preg_Cate', 'id_pregunta');
    }
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Persona(){
        return $this->belongsTo('App\Models\Persona', 'id_persona');
    }
}