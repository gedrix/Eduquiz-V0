<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Opcion extends Model{
    protected $table = 'opcion';
    public $timestamps = false;
    //lista blancas
    protected $fillable = ['opcion', 'estado', 'id_pregunta'];
    //lista negra
    protected $guarded = ['id'];
    
    //Relacion PERNECE A
    public function Pregunta(){
        return $this->belongsTo('App\Models\Pregunta', 'id_pregunta');
    }
}