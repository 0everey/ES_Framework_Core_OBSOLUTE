using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace ES
{
    public class MonoBehaiviourWithCoroutine : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    public static class CoroutineMaker
    {
       public static IEnumerator RepeatConroutine(Action action,float startDelay=1,float internal_=1,int times=-1,CancellationTokenSource source=default)
        {
            
            yield return new WaitForSeconds(startDelay);
            for(int i = 0; i < times || times == -1; i++)
            {
                try
                {
                    if (source != null)
                    {
                        source.Token.ThrowIfCancellationRequested();
                    }
                }
                catch(OperationCanceledException e)
                {
                    Debug.Log("令牌取消" + e);
                    yield break;
                }catch(Exception e2)
                {
                    Debug.Log("其他原因错误" + e2);
                    yield break;
                }

                action?.Invoke();
                yield return new WaitForSeconds(internal_);
            }
        }
        public static IEnumerator DelayCoroutine(Action action,float delay=1, CancellationTokenSource source = default)
        {
            yield return new WaitForSeconds(delay);
            try
            {
                if (source != null)
                {
                    source.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log("令牌取消" + e);
                yield break;
            }
            catch (Exception e2)
            {
                Debug.Log("其他原因错误" + e2);
                yield break;
            }
            action?.Invoke();
        }
        public static IEnumerator DelayOneFrameCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }
        public static IEnumerator RunningConroutine(Action start,Action running,Action end,float time, CancellationTokenSource source = default)
        {
            start?.Invoke();
            while (time > 0)
            {
                yield return new WaitForFixedUpdate();
                time -= Time.deltaTime;
                try
                {
                    if (source != null)
                    {
                        source.Token.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException e)
                {
                    Debug.Log("令牌取消" + e);
                    yield break;
                }
                catch (Exception e2)
                {
                    Debug.Log("其他原因错误" + e2);
                    yield break;
                }
                running?.Invoke();
                
            }

            end?.Invoke();
            yield return null;
        }
    }
}
