using System;

abstract class Sensor {
	
	private String id;
	private String roomId;
	private int floorNo;
	private String sensorName;
	private String departmentName;
    private String sensorData;
    private ReadingModel model;
	
	public Sensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") {
		
		this.id = id;
		this.roomId = roomId;
		this.floorNo = floorNo;
		this.sensorName = sensorName;
		this.departmentName = departmentName;
        this.sensorData = "";
        this.model = new ReadingModel();

        this.model.sensorId = this.id;
        this.model.sensorName = this.sensorName;
        this.model.floorNo = this.floorNo;
        this.model.departmentName = this.departmentName;
        this.model.roomId = this.roomId;
        this.model.readingData = "";
	}

    public String getId() {
        return id;
    }
	
	public abstract void poll() {
		
	}

    public string getPackage() {
        return this.sensorData;
    }
	
}
