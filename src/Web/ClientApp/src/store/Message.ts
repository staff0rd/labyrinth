export interface Message {
    id:                   number;
    sender_id:            number;
    replied_to_id:        number;
    created_at:           string;
    network_id:           number;
    message_type:         string;
    sender_type:          string;
    url:                  string;
    web_url:              string;
    group_id:             number;
    body:                 Body;
    thread_id:            number;
    client_type:          string;
    client_url:           string;
    system_message:       boolean;
    direct_message:       boolean;
    chat_client_sequence: null;
    language:             string;
    notified_user_ids:    number[];
    privacy:              string;
    attachments:          any[];
    liked_by:             LikedBy;
    content_excerpt:      string;
    group_created_id:     number;
}

export interface Body {
    parsed: string;
    plain:  string;
    rich:   string;
}

export interface LikedBy {
    count: number;
    names: any[];
}