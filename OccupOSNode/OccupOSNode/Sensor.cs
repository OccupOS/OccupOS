using System;

abstract class Sensor {
	
	private String id;
	private String roomId;
	private int floorNo;
	private String sensorName;
	private String departmentName;
    protected ReadingModel model;
	
	protected Sensor(String id, String roomId, int floorNo, String sensorName = "", String departmentName = "") {
		
		this.id = id;
		this.roomId = roomId;
		this.floorNo = floorNo;
		this.sensorName = sensorName;
		this.departmentName = departmentName;

        this.model = new ReadingModel {
            sensorId = this.id,
            sensorName = this.sensorName,
            floorNo = this.floorNo,
            departmentName = this.departmentName,
            roomId = this.roomId,
            readingData = ""
        };
	}

    public String getId() {
        return id;
    }

    public abstract String poll();	
}
