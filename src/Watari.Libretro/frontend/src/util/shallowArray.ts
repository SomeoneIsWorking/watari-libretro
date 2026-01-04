import { type ShallowRef, type ShallowReactive, customRef, shallowReactive } from 'vue';

type ShallowArray<T> = ShallowRef<ShallowReactive<T[]>>;

export function shallowArray<T>(arr: T[]): ShallowArray<T> {
    return customRef((track, trigger) => {
        let value = shallowReactive(arr);
        return {
            get() {
                track();
                return value;
            },
            set(newVal: T[]) {
                value = shallowReactive(newVal);
                trigger();
            }
        };
    });
}
