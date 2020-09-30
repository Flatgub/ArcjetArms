using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public Text text;

    public void Show(Vector2 pos, int number, Color color)
    {
        text.text = number.ToString();
        text.color = color;
        text.CrossFadeAlpha(0f, 1f, false);

        RectTransform rect = GetComponent<RectTransform>();

        float off = 8f;
        rect.localPosition = pos;
        rect.localPosition += new Vector3(Random.Range(-off, off), Random.Range(-off, off), 0);

        rect.LeanMoveY(rect.localPosition.y + 10f, 1f).setOnComplete(Die);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
