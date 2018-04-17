
[System.Serializable]
public class CarStats
{
	public int index;
	public bool isAlive;

	// Wheels
	public float turnAngle = 55f;
	public float Accelerate = 750f;
	public float Brake = 400000f;
	public float Shunt = 450f;
	public float spring = 10000f;
	public float forwardSwiftness = 1.0f;
	public float sidewaysSwiftness = 1.7f;
}
