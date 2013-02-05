using System;

abstract class Sensor {
	
	private String id;
	private String roomId;
	private int floorNo;
	private String sensorName;
	private String departmentName;
    private String data;
    private ReadingModel model;
	
	public Sensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") {
		
		this.id = id;
		this.roomId = roomId;
		this.floorNo = floorNo;
		this.sensorName = sensorName;
		this.departmentName = departmentName;
        this.data = "";
        this.model = new ReadingModel();
	}

    public String getId() {
        return id;
    }
	
	public abstract void poll() {
		
	}

    public string getPackage() {
        return this.data;
    }
	
}
