<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class Categoria extends Model{
    protected $table = 'categoria';
    public $timestamps = false;
    //lista blancas
    protected $fillable = ['nombre', 'external_id', 'imagen'];
    //lista negra
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Preg_Cate(){
        return $this->hasMany('App\Models\Preg_Cate', 'id_categoria');
    }
}