using System;

abstract class Sensor {
	
	private String id;

    protected Sensor(String id) {

        this.id = id;
    }

    public String getId() {
        return id;
    }

    public abstract String poll();	
}
