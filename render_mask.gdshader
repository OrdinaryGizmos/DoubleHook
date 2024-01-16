shader_type spatial;
render_mode depth_prepass_alpha,cull_disabled;

uniform bool masked = false;//set to true if passing thru a portal
uniform vec3 Portal_Global_Origin;
uniform vec3 Portal_Size;
uniform vec3 Portal_Rotation;
uniform sampler2D object_texture;
varying float y_f;
varying vec3 world_position;

void vertex() {
	if(masked){
		world_position = VERTEX;	//must have "world_vertex_coords" enabled as render mode for this to work
	}
}

void fragment() {
	if(masked){
		ALPHA_SCISSOR_THRESHOLD = 2.0;
		vec3 offset = world_position - Portal_Global_Origin;
		if(offset.x < Portal_Size.x && offset.x > -Portal_Size.x){
			if(offset.y < Portal_Size.y && offset.y > -Portal_Size.y){
				if(offset.z < Portal_Size.z && offset.z > -Portal_Size.z){
					ALPHA_SCISSOR_THRESHOLD = 0.0;
				}
			}
		}
	}
	ALBEDO = texture(object_texture, UV).rgb;
}
