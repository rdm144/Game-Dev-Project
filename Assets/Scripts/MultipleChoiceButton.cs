using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleChoiceButton : MonoBehaviour
{
    public TextMesh textMesh;
    public int index;
    public MultipleChoiceController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = controller.GetQuestion().answers[index];
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            controller.SubmitAnswer(index);
        }

    }
}
