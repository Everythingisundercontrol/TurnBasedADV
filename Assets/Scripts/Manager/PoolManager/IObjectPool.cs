public interface IObjectPool
{
    void AutoDestroy(); //定期检查并自动销毁闲置时间过长的对象
}