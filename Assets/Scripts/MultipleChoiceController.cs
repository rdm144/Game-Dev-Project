using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleChoiceController : MonoBehaviour
{
    public struct MCQuestion {
        public string question;
        public string[] answers;
        public int correctChoice;
    }

    MCQuestion currentQuestion;
    int hp;

    // Start is called before the first frame update
    void Start()
    {
        hp = 50;
        NewQuestion();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewQuestion() {
        switch (Random.Range(0, 4)) {
            case 0:
                currentQuestion = TrapQuestion();
                break;
            case 1:
                currentQuestion = Question1();
                break;
            case 2:
                currentQuestion = Question2();
                break;
            case 3:
                currentQuestion = Question3();
                break;
           /* case 4:
                currentQuestion = Question4();
                break;*/
        }
    }

    MCQuestion TrapQuestion() {
        return new MCQuestion {
            question = "What is 2 + 2?",
            answers = new string[4] {
                "1", "2", "3", "5"
            },
            correctChoice = 0
        };
    }

    MCQuestion Question1() {
        return new MCQuestion {
            question = "How many days are in a fortnight?",
            answers = new string[4] {
                "4", "9", "14", "40"
            },
            correctChoice = 0b0100
        };
    }

    MCQuestion Question2() {
        return new MCQuestion {
            question = "Do you like the game so far?",
            answers = new string[4] {
                "No", "No", "No", "No"
            },
            correctChoice = 0b1111
        };

    }

    MCQuestion Question3() {
        return new MCQuestion {
            question = "Which is the most common letter in the English language.",
            answers = new string[4] {
                "N", "O", "P", "E"
            },
            correctChoice = 0b1000
        };
 
    }

    MCQuestion Question4() {
        return new MCQuestion {
            question = "Which planet in the solar system rotates in the reverse direction?",
            answers = new string[4] {
                "Mars", "Venus", "Saturn", "Uranus"
            },
            correctChoice = 0b0010
        };
    }

    MCQuestion Question5() {
        return new MCQuestion {
            question = "Which came first, the chicken or the egg.",
            answers = new string[4] {
                "Chicken", "Egg", "Yes", "No"
            },
            correctChoice = 0b0111
        };
    }

    public MCQuestion GetQuestion() {
        return currentQuestion;
    }

    public void SubmitAnswer(int answer) {
        int answer_b = 1 << answer;
        bool correct = (currentQuestion.correctChoice & answer_b) != 0;
        if (correct)
            OnCorrectAnswer();
        else
            OnIncorrectAnswer();
        NewQuestion();
    }

    void OnCorrectAnswer() {
        TakeDamage();
    }

    void OnIncorrectAnswer() {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().TakeDamage(10);
    }

    void TakeDamage() {
        hp -= 10;
        if (hp < 0) {
            OnDeath();
        }
    }

    void OnDeath() {
        StartCoroutine("WaitAndChangeScene");
    }

    IEnumerator WaitAndChangeScene() {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
