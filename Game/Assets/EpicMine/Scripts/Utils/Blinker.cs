using System.Collections;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private Light _light;

    [SerializeField] private float _startValue;
    [SerializeField] private float _endValue;

    [SerializeField] private float _speed=10;

    public void Start()
    {
        StartCoroutine(_startBlink());
    }

    public IEnumerator _startBlink()
    {
        var isDown = true;

        while (true)
        {
            var startValue = Random.Range(_startValue, _endValue);
            var endValue = Random.Range(startValue, _endValue);
            var speed = Random.Range(_speed - _speed * 0.5f, _speed + _speed * 0.5f);

            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (isDown)
                {
                    _light.intensity -= Time.deltaTime * speed;
                    if (_light.intensity <= startValue)
                    break;
                    
                }
                else
                {
                    _light.intensity += Time.deltaTime * speed;
                    if (_light.intensity >= endValue)
                        break;
                }
            }
            isDown = !isDown;
        }
    }

}
