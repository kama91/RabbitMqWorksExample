namespace Core.Serializer.Abstractions;

public interface IJsonByteArraySerializer
{
    byte[] Serialize(object data);

    TData Deserialize<TData>(byte[] bytes);
}