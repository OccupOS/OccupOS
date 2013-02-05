using System;

abstract class Sensor {
	
	private String id;
	private String roomId;
	private int floorNo;
	private String sensorName;
	private String departmentName;
	
	public Sensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") {
		
		this.id = id;
		this.roomId = roomId;
		this.floorNo = floorNo;
		this.sensorName = sensorName;
		this.departmentName = departmentName;
		
	}

    public String getId() {
        return id;
    }
	
	public abstract void poll() {
		
	}

    public void sendPackage() {

    }
	
}
