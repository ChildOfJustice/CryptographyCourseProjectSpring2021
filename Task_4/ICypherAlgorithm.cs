namespace Task_4
{
    public interface ICypherAlgorithm
    {
        byte[] Encrypt(byte[] dataBytes);
        byte[] Decrypt(byte[] dataBytes);
    }
}