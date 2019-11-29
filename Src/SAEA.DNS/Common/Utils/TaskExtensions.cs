/****************************************************************************
*��Ŀ���ƣ�SAEA.DNS
*CLR �汾��3.0
*�������ƣ�WENLI-PC
*�����ռ䣺SAEA.DNS.Common.Utils
*�� �� �ƣ�TaskExtensions
*�� �� �ţ�v5.0.0.1
*�����ˣ� yswenli
*�������䣺wenguoli_520@qq.com
*����ʱ�䣺2019/11/28 22:43:28
*������
*=====================================================================
*�޸�ʱ�䣺2019/11/28 22:43:28
*�� �� �ˣ� yswenli
*�� �� �ţ� v5.0.0.1
*��    ����
*****************************************************************************/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SAEA.DNS.Common.Utils
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration registration = token.Register(src =>
            {
                ((TaskCompletionSource<bool>)src).TrySetResult(true);
            }, tcs);

            using (registration)
            {
                if (await Task.WhenAny(task, tcs.Task) != task)
                {
                    throw new OperationCanceledException(token);
                }
            }

            return await task;
        }

        public static async Task<T> WithCancellationTimeout<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout))
            using (CancellationTokenSource linkSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken))
            {
                return await task.WithCancellation(linkSource.Token);
            }
        }
    }
}
