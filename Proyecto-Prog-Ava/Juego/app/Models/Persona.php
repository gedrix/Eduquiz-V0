<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Persona extends Model{
    protected $table = 'persona';
    public $timestamps = true;
    //lista blancas
    protected $fillable = ['nombre', 'correo', 'clave', 'rol', 'estado', 'external_id', 'imagen', 'created_at', 'updated_at'];
    //lista negra
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Preguntas(){
        return $this->hasMany('App\Models\Pregunta', 'id_persona');
    }
    
    //Relacion UNO a UNO
    public function Nivel_Usuario(){
        return $this->hasOne('App\Models\Nivel_Usuario', 'id_persona');
    }
}