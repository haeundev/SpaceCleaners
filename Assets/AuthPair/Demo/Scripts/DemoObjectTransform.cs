using UnityEngine;
using UnityEngine.UI;

public class DemoObjectTransform : MonoBehaviour
{
    public float speed = 0.5f;
    public float amplitude = 2f;
    private bool _movingDown = false;
    private bool _scalingDown = false;

    public Text scoreText;

    void Update()
    {
        if (transform.localPosition.y > amplitude/2 && !_movingDown)
        {
            _movingDown = true;
            UpdateScore();
        }
        else if(transform.localPosition.y < amplitude/-2 && _movingDown)
        {
            _movingDown = false;
            UpdateScore();
        }

        if (transform.localScale.y > 1 && !_scalingDown)
        {
            _scalingDown = true;
        }
        else if (transform.localScale.y < 0.2 && _scalingDown)
        {
            _scalingDown = false;
        }

        var scale = (_scalingDown ? -0.5f : 0.5f) * speed * Time.deltaTime;
        transform.localPosition += speed * Time.deltaTime * (_movingDown? Vector3.down : Vector3.up);
        transform.Rotate(36 * speed * Time.deltaTime, 0, 0);
        transform.localScale += new Vector3(scale, scale, scale);
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            int.TryParse(scoreText.text, out int score);
            score++;
            scoreText.text = score.ToString();
        }
    }
}