using System.Collections.Generic;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public GameObject card;
    public bool isDealer = false;
    public int points;
    private int coordY;

    public bool HasAces()
    {
        foreach (GameObject card in cards)
        {
            if (card.GetComponent<CardModel>().value == 11)
                return true;
        }
        return false;
    }

    private void Awake()
    {
        points = 0;
        coordY = isDealer ? -1 : 3;
    }

    public void Clear()
    {
        points = 0;
        coordY = isDealer ? -1 : 3;
        foreach (GameObject g in cards)
            Destroy(g);
        cards.Clear();
    }

    public void InitialToggle()
    {
        if (cards.Count > 0)
            cards[0].GetComponent<CardModel>().ToggleFace(true);
    }

public void Push(Sprite front, int value)
{
    GameObject cardCopy = Instantiate(card);
    cards.Add(cardCopy);

    float coordX = 1.4f * (cards.Count - 4);
    cardCopy.transform.position = new Vector3(coordX, coordY);

    CardModel model = cardCopy.GetComponent<CardModel>();
    model.front = front;
    model.value = value;
    
    // Modificado: Solo muestra la cara si no es dealer O es la primera carta del dealer
    bool showFace = !isDealer || (isDealer && cards.Count == 1);
    model.ToggleFace(showFace);

    CalculatePoints();
}

    private void CalculatePoints()
    {
        int val = 0;
        int aces = 0;

        foreach (GameObject f in cards)
        {
            if (f.GetComponent<CardModel>().value != 11)
                val += f.GetComponent<CardModel>().value;
            else
                aces++;
        }

        for (int i = 0; i < aces; i++)
        {
            val += (val + 11 <= 21) ? 11 : 1;
        }

        points = val;
    }
}