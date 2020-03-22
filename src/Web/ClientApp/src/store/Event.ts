export interface Event {
    id: number;
    entity_id: string;	
    event_name: string;
    body: string;
    inserted_at: Date;
    timestamp: Date;
}