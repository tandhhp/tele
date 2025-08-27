import { DrawerForm, DrawerFormProps } from "@ant-design/pro-components"

type Props = DrawerFormProps & {
    reload?: () => void;
}

const ContactForm: React.FC<Props> = (props) => {
    return (
        <DrawerForm {...props} title="Liên hệ">

        </DrawerForm>
    )
}

export default ContactForm;