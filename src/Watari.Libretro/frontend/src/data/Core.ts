import { ref } from 'vue';

type CoreStatus = 'available' | 'downloading' | 'downloaded';

export class Core {
    id: string;
    name: string;
    database: string[];
    status = ref<CoreStatus>('available');
    progress = ref<number>(0);

    constructor(id: string, name: string, database: string[], isDownloaded: boolean) {
        this.id = id;
        this.name = name;
        this.database = database;
        this.status.value = isDownloaded ? 'downloaded' : 'available';
        this.progress.value = 0;
    }

    get isDownloaded() {
        return this.status.value === 'downloaded';
    }

    setDownloaded() {
        this.status.value = 'downloaded';
        this.progress.value = 100;
    }

    setDownloading() {
        this.status.value = 'downloading';
        this.progress.value = 0;
    }

    setProgress(progress: number) {
        this.progress.value = progress;
    }

    setError() {
        this.status.value = 'available';
        this.progress.value = 0;
    }

    setRemoved() {
        this.status.value = 'available';
        this.progress.value = 0;
    }
}
