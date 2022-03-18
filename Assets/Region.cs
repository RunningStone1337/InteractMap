using UnityEngine;
using UnityEngine.EventSystems;

public class Region : MonoBehaviour, IPointerClickHandler
{
    #region Public Methods

    public void OnPointerClick(PointerEventData eventData)
    {
        //создание/открытие окошка с инфой
        //инфа в окошко подгружается по мере получения ответа от сервера
    }

    #endregion Public Methods
}