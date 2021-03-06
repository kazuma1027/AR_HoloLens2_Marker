﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;  // Except

public class Surface_Clear : MonoBehaviour
{
    private GameObject drone;
    public GameObject _building;
    /// <summary>
    /// 今回の Update で検出された遮蔽物の Renderer コンポーネント。
    /// </summary>
    private List<Renderer> rendererHitsList_ = new List<Renderer>();

    /// <summary>
    /// 前回の Update で検出された遮蔽物の Renderer コンポーネント。
    /// 今回の Update で該当しない場合は、遮蔽物ではなくなったので Renderer コンポーネントを有効にする。
    /// </summary>

    /// <summary>
    /// 遮蔽物とするレイヤーマスク。
    /// </summary>
    private int layerMask_;

    /// <summary>
    /// マテリアル比較用
    /// </summary>
    private bool Compare_Material;

    /// <summary>
    /// 半透明マテリアル
    /// </summary>
    private Material mat;
    /// <summary>
    /// Except
    /// </summary>
    private Renderer[] rendererHitsPrevs_;
    /// <summary>
    /// Sphereを出すかの判定
    /// </summary>
    private bool obst = false;
    /// <summary>
    /// 建物のMeshRenderer
    /// </summary>
    Renderer _mesh;


    void Start()
    {
        drone = GameObject.Find("droneModel");
        mat = Resources.Load<Material>("Temparent_obj") as Material;
        if(drone == null){
            Debug.Log("null");
        }
        layerMask_ = 1;
    }

    void Update()
    {   
        // カメラと被写体を結ぶ ray を作成
        Vector3 _difference = (drone.transform.position - this.transform.position);
        Vector3 _direction = _difference.normalized;
        Ray _ray = new Ray(this.transform.position, _direction);
        Quaternion angle = new Quaternion(0f,0f,0f,0f);

        // 前回の結果を退避してから、Raycast して今回の遮蔽物のリストを取得する
        // RaycastHit[] _hits = Physics.RaycastAll(_ray, _difference.magnitude, layerMask_);
        // RaycastHit[] _hits = Physics.SphereCastAll(this.transform.position, 0.03f ,_direction, _difference.magnitude, layerMask_);
        RaycastHit _hit;
        if(Physics.BoxCast(this.transform.position, Vector3.one * 0.2f, _direction, out _hit, angle,  _difference.magnitude, layerMask_)){
            //MaskObjectのRendererコンポーネント

            rendererHitsPrevs_ = rendererHitsList_.ToArray();
            rendererHitsList_.Clear();
            // 遮蔽物は一時的にすべて描画機能を無効にする。
            if(_hit.collider.gameObject.tag == "Building" || _hit.collider.gameObject.tag == "Surface"){
                Debug.Log("1");
                // 遮蔽物の Renderer コンポーネントを無効にする
                Renderer _renderer = _hit.collider.gameObject.GetComponent<Renderer>();
                Material mat = Resources.Load<Material>("Temparent_sur");
                if (_renderer != null)
                {   
                    // _renderer.enabled = false;
                    _renderer.material = mat;
                    rendererHitsList_.Add(_renderer);
                    obst = true;
                    _hit.collider.gameObject.tag = "Surface";

                }
            }

            // 前回まで対象で、今回対象でなくなったものは、表示を元に戻す。
            foreach (Renderer _renderer in rendererHitsPrevs_.Except<Renderer>(rendererHitsList_))
            {
                // Debug.Log("return" + _renderer.material.name);
                // _renderer.enabled = true;
                Compare_Material = (_renderer.material.name).SequenceEqual(mat.name + " (Instance");
                _hit.collider.gameObject.tag = "Building";
                Material mat_default = Resources.Load<Material>("default") as Material;
                _renderer.material = mat_default;
                obst = false;
            }
        }
    }
}
