<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Nivel_Usuario extends Model{
    protected $table = 'nivel_usuario';
    public $timestamps = false;
    //lista blancas
    protected $fillable = ['nivel', 'experiencia', 'clasificacion', 'puntaje', 'id_persona'];
    //lista negra
    protected $guarded = ['id'];
    
    //Relacion PERNECE A
    public function Persona(){
        return $this->belongsTo('App\Models\Persona', 'id_persona');
    }
}