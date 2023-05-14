using System;
using UnityEngine;
using UnityEngine.UI;

namespace ToDelete
{
    public class CubeRotator : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1f;
        
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            _image.transform.rotation = Quaternion.Euler(new Vector3(_image.transform.rotation.eulerAngles.x,
                _image.transform.rotation.eulerAngles.y,
                _image.transform.rotation.eulerAngles.z + _speed * Time.deltaTime));
        }
    }
}