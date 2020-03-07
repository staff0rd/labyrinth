export interface Message {
    bodyParsed: string;
    bodyPlain: string;
    createdAt: Date;
    id: string;
    network: string;
    senderId: string;
}