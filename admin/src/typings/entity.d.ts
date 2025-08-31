export interface BaseEntity<T> {
    id: T;
}

export interface Province extends BaseEntity<number> {
    name: string;
}

export interface JobKind extends BaseEntity<number> {
    name: string;
    isActive: boolean;
}