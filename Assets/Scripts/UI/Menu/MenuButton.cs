using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    private Animator mAnimator;

    // --------------------------------------------------------
    void Awake()
    {
        mAnimator = GetComponent<Animator>();
    }

    // --------------------------------------------------------

    public void PlayHoverSound()
    {
        AudioManager.Instance.PlaySFX_BtnHover();
    }

    // Update is called once per frame
    public void PlayPressedSound()
    {
        AudioManager.Instance.PlaySFX_BtnHover();
    }
}
