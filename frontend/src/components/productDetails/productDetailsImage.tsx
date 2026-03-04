import ImageLightbox from '../../shared/components/ui/imageLightbox.tsx';
import ImagePlaceholder from '../../shared/components/ui/imagePlaceholder.tsx';

export default function ProductDetailImage({
	thumbnailUrl,
	name,
}: {
	thumbnailUrl: string | null | undefined;
	name: string;
}) {
	return (
		<>
			{thumbnailUrl && (
				<ImageLightbox
					src={thumbnailUrl}
					alt={name}
					borderRadius={3}
					boxShadow={3}
				/>
			)}
			{!thumbnailUrl && <ImagePlaceholder fontSize={70} />}
		</>
	);
}
