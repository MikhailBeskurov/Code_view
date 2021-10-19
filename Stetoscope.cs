using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stetoscope : MonoBehaviour
{
    [SerializeField]
    private Canvas parentCanvas = null;
    [SerializeField]
    private RectTransform stetoscope = null;
    [SerializeField]
    private ParticleSystem particle_good = null;
    [SerializeField]
    private ParticleSystem particle_bad= null;
    [SerializeField]
    private Health_status hp = null;

    bool active_enter = false;

    
    bool on_trigger = true;
    bool exit_trigger = false;

    private void Start()
    {
        trigger_stetoscope.trigger += trigger;
    }
    public void Enter()
    {
        active_enter = true;
        particle_good.Play();
    }
    public void Exit()
    {
       
        active_enter = false;
        particle_good.Stop();
        particle_bad.Stop();    
    }

    public IEnumerator Particle() {
        if(hp.Good_Healt)
        yield return new WaitForSeconds(0.1f);
        else yield return new WaitForSeconds(0.55f);
        if (hp.Good_Healt)
            particle_good.Play();
        else
            particle_bad.Play();
    }

    void trigger(bool obj) {
        on_trigger = obj;
    }
    IEnumerator lerp_on_trigger() {
      //  bool active = false;
       // active = active_enter;
       // active_enter = false;

        while (!on_trigger) {
            stetoscope.position = Vector3.Lerp(stetoscope.position, Vector3.zero, 1f * Time.deltaTime);
            yield return null;
        }
        active_enter = false;
        exit_trigger = false;
    }
    float time = 0f;
    bool particle_good_isPlaying = false;
    void Update()
    {
        if (active_enter && on_trigger)
        {
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);
            stetoscope.position = Vector3.Lerp(stetoscope.position, parentCanvas.transform.TransformPoint(movePos), 30f * Time.deltaTime);

            time += Time.deltaTime;
            if (time >= Random.Range(3f,5f))
            {
                if (hp.Good_Healt && !particle_good_isPlaying)
                {
                    particle_bad.Stop();
                    StartCoroutine(Particle());
                    particle_good_isPlaying = true;
                }
                if(!hp.Good_Healt && particle_good_isPlaying)
                {
                    particle_good.Stop();
                    StartCoroutine(Particle());
                    particle_good_isPlaying = false;
                }
                time = 0;
            }
        }
        if (!on_trigger && !exit_trigger)
        {
            StartCoroutine(lerp_on_trigger());
            exit_trigger = true;
        }
    }
}
