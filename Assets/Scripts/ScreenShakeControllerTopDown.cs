using UnityEngine;

public class ScreenShakeControllerTopDown : MonoBehaviour
{
    public static ScreenShakeControllerTopDown instance;
    private float shakeTimeRemaining;
    private float shakePower;
    private float shakeFadeTime;
    private float shakeRotation;
    [SerializeField] private float rotationMultiplier;
    private Vector3 pos;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        pos = transform.position;
    }

    private void LateUpdate()
    {
        if(shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;

            transform.position += new Vector3(xAmount, yAmount, 0f);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
        transform.position = pos;
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power; 

        shakeFadeTime = power/length;
        shakeRotation = power * rotationMultiplier;
    }
}
