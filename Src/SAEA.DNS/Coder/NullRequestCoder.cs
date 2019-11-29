/****************************************************************************
*��Ŀ���ƣ�SAEA.DNS
*CLR �汾��3.0
*�������ƣ�WENLI-PC
*�����ռ䣺SAEA.DNS.Coder
*�� �� �ƣ�NullRequestResolver
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
using SAEA.DNS.Model;
using SAEA.DNS.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace SAEA.DNS.Coder
{
    public class NullRequestCoder : IRequestCoder
    {
        public Task<IResponse> Resolve(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new ResponseException("Request failed");
        }
    }
}
