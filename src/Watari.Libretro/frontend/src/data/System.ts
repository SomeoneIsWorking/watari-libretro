import { ref } from 'vue';

export class System {
    Name: string;
    Extensions: string[];
    CoverName: string;
    CoverData = ref<string>();

    constructor(name: string, extensions: string[], coverName: string) {
        this.Name = name;
        this.Extensions = extensions;
        this.CoverName = coverName;
        this.CoverData.value = undefined;
    }

    setCoverData(data: string | undefined) {
        this.CoverData.value = data;
    }

    getCoverSrc() {
        return this.CoverData.value && `data:image/png;base64,${this.CoverData.value}`;
    }
}