<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Preg_Cate extends Model{
    protected $table = 'preg_cate';
    public $timestamps = false;
    //lista blancas
    protected $fillable = ['id_pregunta', 'id_categoria'];
    
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Pregunta(){
        return $this->belongsTo('App\Models\Pregunta', 'id_pregunta');
    }
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Categoria(){
        return $this->belongsTo('App\Models\Categoria', 'id_categoria');
    }
}